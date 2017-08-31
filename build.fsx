#r "packages/FAKE/tools/FakeLib.dll"
#load "src/app.fsx"
open Fake

#if INTERACTIVE
System.Environment.CurrentDirectory <- __SOURCE_DIRECTORY__
#endif

let code = """
$r "../packages/Microsoft.Azure.WebJobs/lib/net45/Microsoft.Azure.WebJobs.Host.dll"
$r "../packages/Microsoft.AspNet.WebApi.Client/lib/net45/System.Net.Http.Formatting.dll"
$r "../packages/Microsoft.AspNet.WebApi.Core/lib/net45/System.Web.Http.dll"
$r "System.Net.Http"
$load "../src/app.fsx"
$load "../src/lambda/azure.fs"
open Lambda
open System.Net.Http
open Microsoft.Azure.WebJobs.Host

let Run (req:HttpRequestMessage, log:TraceWriter) =
  Lambda.Azure.runFunction "xxxxxx" App.app req log
"""

let config = """
{ "bindings": [
    { "authLevel": "anonymous",
      "name": "req",
      "type": "httpTrigger",
      "direction": "in" },
    { "name": "res",
      "type": "http",
      "direction": "out" } ],
  "disabled": false
}"""

Target "generate" (fun _ ->  
  for func in App.app do
    CleanDir func.Name
    WriteStringToFile false (func.Name </> "run.fsx") (code.Replace("xxxxxx", func.Name).Replace("$","#"))
    WriteStringToFile false (func.Name </> "function.json") config
)

Target "run" (fun _ ->  
  ()
)

RunTargetOrDefault "generate"