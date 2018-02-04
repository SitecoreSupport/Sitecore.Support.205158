using Sitecore.ContentTesting.Inspectors;
using Sitecore.ContentTesting.Pipelines.GetTests;
using Sitecore.Diagnostics;
using Sitecore.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.ContentTesting.Tests;

namespace Sitecore.Support.ContentTesting.Pipelines.GetTests
{
  public class GetPersonalizationTests : GetTestsProcessor
  {
    public override void Process(GetTestsArgs args)
    {
      Assert.ArgumentNotNull(args.Item, "args.Item");
      foreach (RenderingDefinition current in new TestingRenderingInspector(args.Item, null)
      {
        InspectionMode = TestingRenderingInspector.Mode.Personalization,
        DeviceId = args.DeviceId,
        EnsureTestIsNotRunning = true
      }.GetRenderings())
      {
        args.AddTest(new Sitecore.Support.ContentTesting.Tests.PersonalizationTest(current, args.Item));
      }
    }
  }
}
