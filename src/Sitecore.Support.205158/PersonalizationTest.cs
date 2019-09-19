// © 2014-2019 Sitecore Corporation A/S. All rights reserved. Sitecore® is a registered trademark of Sitecore Corporation A/S.
namespace Sitecore.Support.ContentTesting.Tests
{
  using System.Linq;
  using Sitecore.ContentTesting;
  using Sitecore.ContentTesting.Model.Data.Items;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Layouts;
  using Sitecore.Globalization;
  using Sitecore.StringExtensions;
  using Sitecore.ContentTesting.Tests;

  /// <summary>
  /// Represents a single personalization test.
  /// </summary>
  public class PersonalizationTest : RenderingTest, ITest
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="PersonalizationTest"/> class.
    /// </summary>
    /// <param name="renderingDefinition">The rendering using the test.</param>
    /// <param name="contentItem">The Item containing the rendering.</param>
    public PersonalizationTest(RenderingDefinition renderingDefinition, Item contentItem)
      : base(renderingDefinition, contentItem)
    {
    }

    /// <summary>
    /// Gets a friendly name for the test.
    /// </summary>
    public string Name
    {
      get
      {
        return Translate.Text(Texts.PERSONALIZATION_TEST_ON_0).FormatWith(this.RenderingName);
      }
    }

    /// <summary>
    /// Gets the <see cref="TestDefinitionItem"/> for the test.
    /// </summary>
    /// <param name="rendering">The rendering using the test.</param>
    /// <param name="database">The database to lookup data in.</param>
    /// <returns>The test definition item if found, otherwise null.</returns>
    protected override TestDefinitionItem GetTestDefinitionItem(RenderingDefinition rendering, Database database)
    {
      var renderingRef = (from d in this.contentItem.Database.Resources.Devices.GetAll()
                          from r in this.contentItem.Visualization.GetRenderings(d, false)
                          where r.UniqueId == rendering.UniqueId
                          select r).FirstOrDefault();

      if (renderingRef != null)
      {
        var personalizationTest = renderingRef.Settings.PersonalizationTest;

        if (!string.IsNullOrEmpty(personalizationTest))
        {
          var testItem = renderingRef.RenderingItem.Database.GetItem(personalizationTest);
          if (testItem != null)
          {
            return TestDefinitionItem.Create(testItem);
          }
        }
      }

      return null;
    }

    /// <summary>
    /// Remove the test from the rendering.
    /// </summary>
    /// <param name="rendering">The rendering to remove the test from.</param>
    protected override void RemoveFromRendering(RenderingDefinition rendering)
    {
      rendering.PersonalizationTest = null;
    }
  }
}
