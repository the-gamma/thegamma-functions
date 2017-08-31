#r "packages/FAKE/tools/FakeLib.dll"
open Fake

let code = """
#r "../packages/Microsoft.Azure.WebJobs/lib/net45/Microsoft.Azure.WebJobs.Host.dll"
#r "../packages/Microsoft.AspNet.WebApi.Client/lib/net45/System.Net.Http.Formatting.dll"
#r "../packages/Microsoft.AspNet.WebApi.Core/lib/net45/System.Web.Http.dll"
#r "System.Net.Http"

open System.Net
open System.Net.Http
open Microsoft.Azure.WebJobs.Host

let Run(req: HttpRequestMessage, log: TraceWriter) =
  req.CreateResponse(HttpStatusCode.OK, "Totally")
"""

let config = """
{
  "bindings": [
    {
      "authLevel": "anonymous",
      "name": "req",
      "type": "httpTrigger",
      "direction": "in"
    },
    {
      "name": "res",
      "type": "http",
      "direction": "out"
    }
  ],
  "disabled": false
}"""

Target "generate" (fun _ ->  
  for func in ["experiment"] do
    CleanDir func
    WriteStringToFile false (func </> "run.fsx") code
    WriteStringToFile false (func </> "function.json") config
)

RunTargetOrDefault "generate"

(*
    async {
        log.Info(sprintf 
            "F# HTTP trigger function processed a request.")

        let ctx = 
          let req = Unchecked.defaultof<HttpRequest>
          let res = Unchecked.defaultof<HttpResult>
          let rtm = Unchecked.defaultof<HttpRuntime>
          let cnn = Unchecked.defaultof<_>
          { request = req
            runtime = rtm
            connection = cnn 
            userState = Map.empty
            response = res }

        // Set name to query string
        let name =
            req.GetQueryNameValuePairs()
            |> Seq.tryFind (fun q -> q.Key = "name")

        match name with
        | Some x ->
            return req.CreateResponse(HttpStatusCode.OK, Helper.zzz + " " + x.Value);
        | None ->
            let! data = req.Content.ReadAsStringAsync() |> Async.AwaitTask

            if not (String.IsNullOrEmpty(data)) then
                let named = JsonConvert.DeserializeObject<Message>(data)
                return req.CreateResponse(HttpStatusCode.OK, "Hello " + named.name);
            else
                return req.CreateResponse(HttpStatusCode.BadRequest, "Specify a Name value");
    } |> Async.RunSynchronously
                *)
