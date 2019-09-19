// © 2014-2019 Sitecore Corporation A/S. All rights reserved. Sitecore® is a registered trademark of Sitecore Corporation A/S.
namespace Sitecore.Support.ContentTesting.Pipelines.GetTests
{
  using Sitecore.ContentTesting.Inspectors;
  using Sitecore.ContentTesting.Pipelines.GetTests;
  using Sitecore.ContentTesting.Tests;
  using Sitecore.Diagnostics;

  /// <summary>
  /// Processor to identify component tests for an item.
  /// </summary>
  public class GetComponentTests : GetTestsProcessor
  {
    /// <summary>
    /// Executes this processor.
    /// </summary>
    /// <param name="args">The arguments to process.</param>
    public override void Process(GetTestsArgs args)
    {
      Assert.ArgumentNotNull(args.Item, "args.Item");

      var inspector = new TestingRenderingInspector(args.Item)
      {
        InspectionMode = TestingRenderingInspector.Mode.Component,
        DeviceId = args.DeviceId,
        EnsureTestIsNotRunning = true
      };

      var renderings = inspector.GetRenderings();
        
      foreach(var rendering in renderings)
      {
        args.AddTest(new Sitecore.Support.ContentTesting.Tests.ComponentTest(rendering, args.Item));
      }
    }
  }
}
