#r "packages/FSharp.Data/lib/net40/FSharp.Data.dll"

open System
open System.Net
open FSharp.Data
open FSharp.Data.JsonExtensions

type Jira = {
    Server : string
    Api : string
    User : string
    Password : string
}

let jira_search (jira: Jira) jql max_results =
  let url = sprintf "%s/%s/search?%s&fields=id,key,summary&maxResults=%i" jira.Server jira.Api jql max_results
  Http.RequestString(url,
    httpMethod = HttpMethod.Get,
    headers = [
      HttpRequestHeaders.UserAgent ""
      HttpRequestHeaders.ContentType HttpContentTypes.Json
      HttpRequestHeaders.BasicAuth jira.User jira.Password])
  |> JsonValue.Parse
