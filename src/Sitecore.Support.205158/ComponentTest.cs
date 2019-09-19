// © 2014-2019 Sitecore Corporation A/S. All rights reserved. Sitecore® is a registered trademark of Sitecore Corporation A/S.
namespace Sitecore.Support.ContentTesting.Tests
{
  using Sitecore.ContentTesting;
  using Sitecore.ContentTesting.Data;
  using Sitecore.ContentTesting.Model.Data.Items;
  using Sitecore.ContentTesting.Tests;
  using Sitecore.Data;
  using Sitecore.Data.Items;
  using Sitecore.Globalization;
  using Sitecore.Layouts;
  using Sitecore.StringExtensions;

  /// <summary>
  /// Represents a single component test.
  /// </summary>
  public class ComponentTest : RenderingTest, ITest
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentTest"/> class.
    /// </summary>
    /// <param name="renderingDefinition">The rendering using the test.</param>
    /// <param name="contentItem">The Item containing the rendering.</param>
    /// <param name="testStore">The content test store to read test data from.</param>
    public ComponentTest(RenderingDefinition renderingDefinition, Item contentItem, SitecoreContentTestStore testStore = null)
      : base(renderingDefinition, contentItem, testStore)
    {
    }

    /// <summary>
    /// Gets a friendly name for the test.
    /// </summary>
    public string Name
    {
      get
      {
        return Translate.Text(Texts.COMPONENT_TEST_ON_0).FormatWith(this.RenderingName);
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
      var variableItem = this.contentTestStore.GetMultivariateTestVariable(rendering, this.contentItem.Language, database);
      if (variableItem != null)
      {
        return variableItem.TestDefinition;
      }

      return null;
    }

    /// <summary>
    /// Remove the test from the rendering.
    /// </summary>
    /// <param name="rendering">The rendering to remove the test from.</param>
    protected override void RemoveFromRendering(RenderingDefinition rendering)
    {
      rendering.MultiVariateTest = null;
    }
  }
}
