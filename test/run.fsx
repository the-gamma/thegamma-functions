#if !COMPILED
#r "../packages/Microsoft.Azure.WebJobs/lib/net45/Microsoft.Azure.WebJobs.Host.dll"
#r "../packages/Newtonsoft.Json/lib/net45/Newtonsoft.Json.dll"
#r "../packages/Microsoft.AspNet.WebApi.Client/lib/net45/System.Net.Http.Formatting.dll"
#r "../packages/Microsoft.AspNet.WebApi.Core/lib/net45/System.Web.Http.dll"
#r "System.Net.Http"
#r "Newtonsoft.Json"
#r @"..\packages\Suave\lib\net40\Suave.dll"
#else
module MyAzureFunction
#endif

open System.Net
open System.Net.Http
open Newtonsoft.Json
open Microsoft.Azure.WebJobs.Host
open System

type Message = {
    name : string
}

open Suave.Http

let Run(req: HttpRequestMessage, log: TraceWriter) =
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
            return req.CreateResponse(HttpStatusCode.OK, "Hello " + x.Value);
        | None ->
            let! data = req.Content.ReadAsStringAsync() |> Async.AwaitTask

            if not (String.IsNullOrEmpty(data)) then
                let named = JsonConvert.DeserializeObject<Message>(data)
                return req.CreateResponse(HttpStatusCode.OK, "Hello " + named.name);
            else
                return req.CreateResponse(HttpStatusCode.BadRequest, "Specify a Name value");
    } |> Async.RunSynchronously
