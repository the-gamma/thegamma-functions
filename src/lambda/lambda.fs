namespace Lambda

type HttpInput = 
  { RequestUri : System.Uri
    Method : string
    Query : Map<string, string> 
    Headers : Map<string, string list> }

type HttpOutput = 
  { Status : int
    Content : string
    Encoding : System.Text.Encoding
    Headers : Map<string, string list>
    ContentType : string  }

type FunctionOutput<'TIn> = 
  | HttpOutput of ('TIn -> HttpOutput)

type FunctionInput =
  | HttpInput of FunctionOutput<HttpInput>

type Function = 
  { Name : string
    InputOutput : FunctionInput }

[<AutoOpen>]
module Helpers =
  let func name inputf outputf f = 
    { Name = name; InputOutput = inputf(outputf(f)) }
