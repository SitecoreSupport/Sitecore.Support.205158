// © 2014-2019 Sitecore Corporation A/S. All rights reserved. Sitecore® is a registered trademark of Sitecore Corporation A/S.
namespace Sitecore.Support.ContentTesting.Tests
{
  using Sitecore.ContentTesting;
  using Sitecore.ContentTesting.Data;
  using Sitecore.ContentTesting.Model.Data.Items;
  using Sitecore.Data;
  using Sitecore.Data.Fields;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Layouts;
  using Sitecore.Xml;

  /// <summary>
  /// Represents a single test.
  /// </summary>
  public abstract class RenderingTest
  {
    /// <summary>The rendering using the test.</summary>
    protected RenderingDefinition renderingDefinition = null;

    /// <summary>The item containing the rendering.</summary>
    protected Item contentItem = null;

    protected string Layout { get; set; }
    protected string Delta { get; set; }


    /// <summary>The content test store to read test data from.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104: Do not declare read only mutable reference types", Justification = "readonly makes the contentTestStore immutable.")]
    protected readonly SitecoreContentTestStore contentTestStore = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="RenderingTest"/> class.
    /// </summary>
    /// <param name="renderingDefinition">The rendering using the test.</param>
    /// <param name="contentItem">The item containing the rendering.</param>
    /// <param name="testStore">The content test store to read test data from.</param>
    protected RenderingTest(RenderingDefinition renderingDefinition, Item contentItem, SitecoreContentTestStore testStore = null)
    {
      this.renderingDefinition = renderingDefinition;
      this.contentItem = contentItem;

      this.contentTestStore =
        testStore ??
        ContentTestingFactory.Instance.ContentTestStore as SitecoreContentTestStore ??
        new SitecoreContentTestStore();
    }

    /// <summary>
    /// Gets the name of the rendering.
    /// </summary>
    public string RenderingName
    {
      get
      {
        var rendering = this.contentItem.Database.Resources.Renderings[this.renderingDefinition.ItemID];
        if (rendering != null)
        {
          return rendering.DisplayName;
        }
        else
        {
          return this.renderingDefinition.ItemID.ToString();
        }
      }
    }

    /// <summary>
    /// Removes the test.
    /// </summary>
    /// <param name="deleteItems">Indicates whether the test definition item should be deleted or recycled.</param>
    /// <returns>True if successful, otherwise False.</returns>
    public bool Remove(bool deleteItems)
    {
      var testDefinition = this.GetTestDefinitionItem(this.renderingDefinition, this.contentItem.Database);
      this.Layout = LayoutField.GetFieldValue(this.contentItem.Fields[FieldIDs.LayoutField]);
      string delta;
      if ((delta = this.contentItem.Fields[FieldIDs.FinalLayoutField].GetValue(false, false)) == null)
      {
        delta = (this.contentItem.Fields[FieldIDs.FinalLayoutField].GetInheritedValue(false) ?? this.contentItem.Fields[FieldIDs.FinalLayoutField].GetValue(false, false, true, false, false));
      }
      this.Delta = delta;
      this.RemoveInField(FieldIDs.LayoutField);
      this.RemoveInField(FieldIDs.FinalLayoutField);

      if (testDefinition != null)
      {
        if (deleteItems)
        {
          testDefinition.InnerItem.Delete();
        }
        else
        {
          testDefinition.InnerItem.Recycle();
        }
      }

      return true;
    }

    /// <summary>
    /// Remove the test from the rendering.
    /// </summary>
    /// <param name="rendering">The rendering to remove the test from.</param>
    protected abstract void RemoveFromRendering(RenderingDefinition rendering);

    /// <summary>
    /// Gets the <see cref="TestDefinitionItem"/> for the test.
    /// </summary>
    /// <param name="rendering">The rendering using the test.</param>
    /// <param name="database">The database to lookup data in.</param>
    /// <returns>The test definition item if found, otherwise null.</returns>
    protected abstract TestDefinitionItem GetTestDefinitionItem(RenderingDefinition rendering, Database database);

    /// <summary>
    /// Remove the test from a single field.
    /// </summary>
    /// <param name="fieldId">The ID of the field to process.</param>
    protected void RemoveInField(ID fieldId)
    {
      // Ensure field has content
      if (string.IsNullOrWhiteSpace(this.contentItem[fieldId]))
      {
        return;
      }

      //var layoutData = LayoutField.GetFieldValue(this.contentItem.Fields[fieldId]);
      //var layout = LayoutDefinition.Parse(layoutData);
      var layout = fieldId == FieldIDs.LayoutField ? LayoutDefinition.Parse(this.Layout) : LayoutDefinition.Parse(this.FinalLayout);  // LayoutDefinition.Parse(LayoutField.GetFieldValue(this.contentItem.Fields[fieldId]));
      
      if (layout == null)
      {
        return;
      }

      foreach (DeviceDefinition device in layout.Devices)
      {
        var deviceDefinition = layout.GetDevice(device.ID.ToString());
        if (deviceDefinition == null)
        {
          continue;
        }

        var rendering = deviceDefinition.GetRenderingByUniqueId(this.renderingDefinition.UniqueId);
        if (rendering == null)
        {
          continue;
        }

        //rendering.MultiVariateTest = null;
        this.RemoveFromRendering(rendering);
      }      

      if (fieldId == FieldIDs.LayoutField)
      {
        this.Layout = layout.ToXml();
        using (new EditContext(this.contentItem))
        {
          LayoutField.SetFieldValue(this.contentItem.Fields[fieldId], this.Layout);
        }
      }
      else
      {
        this.FinalLayout = layout.ToXml();
        using (new EditContext(this.contentItem))
        {
          LayoutField.SetFieldValue(this.contentItem.Fields[fieldId], this.FinalLayout, this.Layout);
        }
      }

    }
    protected string FinalLayout
    {
      get
      {
        string layoutDelta = this.Delta;
        if (string.IsNullOrWhiteSpace(layoutDelta))
        {
          return this.Layout;
        }
        if (string.IsNullOrWhiteSpace(this.Layout))
        {
          return layoutDelta;
        }
        return XmlDeltas.ApplyDelta(this.Layout, layoutDelta);
      }
      set
      {
        Assert.ArgumentNotNull(value, "value");
        if (!string.IsNullOrWhiteSpace(this.Layout))
        {
          this.Delta = (XmlUtil.XmlStringsAreEqual(this.Layout, value) ? null : XmlDeltas.GetDelta(value, this.Layout));
          return;
        }
        this.Delta = value;
      }
    }

  }
}
