#r "packages/FSharp.Data/lib/net40/FSharp.Data.dll"

namespace JiraHelper

open System
open System.Net
open FSharp.Data
open FSharp.Data.JsonExtensions

[<AutoOpen>]
module Jira =
  type Jira = {
        Server : string
        Api : string
        User : string
        Password : string }

  type Issue = {
        Id : string
        Summary : string
        Url : string }


  let BuildHeaders (jira: Jira) =
    let headers = [
      HttpRequestHeaders.UserAgent ""
      HttpRequestHeaders.ContentType HttpContentTypes.Json
    ]

    if jira.User <> "" && jira.Password <> ""
      then headers |> List.append [HttpRequestHeaders.BasicAuth jira.User jira.Password ]
      else headers

  let Search jql max_results (jira: Jira) =
    let url = sprintf "%s/%s/search" jira.Server jira.Api
    let result =
      Http.RequestString(url,
        query   = [
          "jql", jql;
          "fields", "id,key,summary";
          "maxResults", sprintf "%i" max_results ],
        httpMethod = HttpMethod.Get,
        headers = BuildHeaders jira )
      |> JsonValue.Parse

    let total = result?total.AsInteger()
    if total > max_results
      then printfn "%i issues matched but only %i will be returned" total max_results

    result?issues.AsArray()
      |> Seq.map (fun issue ->
        { Id = issue?key.AsString()
          Summary = issue?fields?summary.AsString()
          Url = issue?self.AsString() })
