#load "lambda/lambda.fs"
open Lambda

let app = 
  [ func "demo" HttpInput HttpOutput (fun input ->
      let name = defaultArg (Map.tryFind "name" input.Query) "anonymous"
      { Status = 200
        Encoding = System.Text.Encoding.UTF8
        Content = "Hello " + name
        Headers = Map.empty
        ContentType = "text/plain" })
  ]
