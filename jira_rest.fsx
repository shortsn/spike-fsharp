#r "packages/FSharp.Data/lib/net40/FSharp.Data.dll"

namespace JiraHelper

open System
open System.Net
open FSharp.Data
open FSharp.Data.JsonExtensions

[<AutoOpen>]
module Jira =
  type Client = {
      Server : string
      Api : string
      User : string
      Password : string
  }

  let BuildHeaders (jira: Client) =
    let headers = [
      HttpRequestHeaders.UserAgent ""
      HttpRequestHeaders.ContentType HttpContentTypes.Json
    ]

    if jira.User <> "" && jira.Password <> ""
      then headers |> List.append [HttpRequestHeaders.BasicAuth jira.User jira.Password ]
      else headers

  let Search (jira: Client) jql max_results =
    let url = sprintf "%s/%s/search?jql=%s&fields=id,key,summary&maxResults=%i" jira.Server jira.Api jql max_results
    Http.RequestString(url,
      httpMethod = HttpMethod.Get,
      headers = BuildHeaders jira
      )
    |> JsonValue.Parse

  type Client with
    member this.Search = Search this
