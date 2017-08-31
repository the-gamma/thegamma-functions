module Lambda.Azure
open Lambda
open System.Net
open System.Net.Http
open Microsoft.Azure.WebJobs.Host

let runFunction name (app:Lambda.Function list) (req:HttpRequestMessage) log = 
  let func = app |> List.tryFind (fun f -> f.Name = name)
  match func with 
  | None -> 
      req.CreateErrorResponse(HttpStatusCode.InternalServerError, sprintf "Function '%s' not found." name)
  | Some { InputOutput = HttpInput(HttpOutput f) } ->
      let input = 
        { RequestUri = req.RequestUri
          Method = req.Method.ToString()
          Query = Map.ofSeq [for q in req.GetQueryNameValuePairs() -> q.Key, q.Value ] 
          Headers = Map.ofSeq [for h in req.Headers -> h.Key, List.ofSeq h.Value ] }
      let output = f input    
      let res = new HttpResponseMessage(unbox output.Status, Content=new StringContent(output.Content, output.Encoding, output.ContentType))
      for hk, hv in Map.toSeq output.Headers do res.Headers.Add(hk, hv)
      res 
