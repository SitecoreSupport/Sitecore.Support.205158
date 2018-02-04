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

    bool ITest.Remove(bool deleteItems)
    {
      TestDefinitionItem testDefinitionItem = this.GetTestDefinitionItem(this.renderingDefinition, this.contentItem.Database);
      this.RemoveInField(FieldIDs.LayoutField);
      this.RemoveInField(FieldIDs.FinalLayoutField);
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
  }
}