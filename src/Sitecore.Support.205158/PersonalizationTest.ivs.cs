using System;
using Sitecore.ContentTesting.Tests;
using Sitecore.Data.Items;
using Sitecore.Layouts;
using Sitecore.ContentTesting.Model.Data.Items;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Diagnostics;
using Sitecore.Xml;

namespace Sitecore.Support.ContentTesting.Tests
{
  public class PersonalizationTest : Sitecore.ContentTesting.Tests.PersonalizationTest, ITest
  {
    public PersonalizationTest(RenderingDefinition renderingDefinition, Item contentItem) : base(renderingDefinition, contentItem)
    {
    }

    protected string Layout { get; set; }
    protected string Delta { get; set; }

    bool ITest.Remove(bool deleteItems)
    {
      TestDefinitionItem testDefinitionItem = this.GetTestDefinitionItem(this.renderingDefinition, this.contentItem.Database);
      this.Layout = LayoutField.GetFieldValue(this.contentItem.Fields[FieldIDs.LayoutField]);
      string delta;
      if ((delta = this.contentItem.Fields[FieldIDs.FinalLayoutField].GetValue(false, false)) == null)
      {
        delta = (this.contentItem.Fields[FieldIDs.FinalLayoutField].GetInheritedValue(false) ?? this.contentItem.Fields[FieldIDs.FinalLayoutField].GetValue(false, false, true, false, false));
      }
      this.Delta = delta;
      this.RemoveInFieldEx(FieldIDs.LayoutField);
      this.RemoveInFieldEx(FieldIDs.FinalLayoutField);
      if (testDefinitionItem != null)
      {
        if (deleteItems)
        {
          testDefinitionItem.InnerItem.Delete();
        }
        else
        {
          testDefinitionItem.InnerItem.Recycle();
        }
      }
      return true;
    }

    private void RemoveInFieldEx(ID fieldId)
    {
      if (string.IsNullOrWhiteSpace(this.contentItem[fieldId]))
      {
        return;
      }
      LayoutDefinition layoutDefinition = fieldId == FieldIDs.LayoutField ? LayoutDefinition.Parse(this.Layout) : LayoutDefinition.Parse(this.FinalLayout);  // LayoutDefinition.Parse(LayoutField.GetFieldValue(this.contentItem.Fields[fieldId]));
      if (layoutDefinition == null)
      {
        return;
      }
      foreach (DeviceDefinition deviceDefinition in layoutDefinition.Devices)
      {
        DeviceDefinition device = layoutDefinition.GetDevice(deviceDefinition.ID.ToString());
        if (device != null)
        {
          RenderingDefinition renderingByUniqueId = device.GetRenderingByUniqueId(this.renderingDefinition.UniqueId);
          if (renderingByUniqueId != null)
          {
            this.RemoveFromRendering(renderingByUniqueId);
          }
        }
      }

      if (fieldId == FieldIDs.LayoutField)
      {
        this.Layout = layoutDefinition.ToXml();
        using (new EditContext(this.contentItem))
        {
          LayoutField.SetFieldValue(this.contentItem.Fields[fieldId], this.Layout);
        }
      }
      else
      {
        this.FinalLayout = layoutDefinition.ToXml();
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