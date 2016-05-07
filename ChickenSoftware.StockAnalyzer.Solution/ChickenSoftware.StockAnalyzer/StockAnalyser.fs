namespace ChickenSoftware.StockAnalyzer

open System
open Accord
open FSharp.Data
open System.Net.Http
open Accord.Statistics
open Accord.Statistics.Models.Regression.Linear

type YahooContext = CsvProvider<"http://ichart.finance.yahoo.com/table.csv?s=MSFT">
type LuisContext = JsonProvider<"../data/Luis.json">

type public StockAnalyser() = 
    member public this.GetStockClose ticker = 
        try
            let stockInfo = YahooContext.Load("http://ichart.finance.yahoo.com/table.csv?s=" + ticker)
            let mostRecent = stockInfo.Rows |> Seq.head
            (float)mostRecent.``Adj Close``
        with 
            | :?  System.Net.WebException -> -1.0

    member public this.GetPricePrediction ticker (targetDate:DateTime) =
        let stockInfo = YahooContext.Load("http://ichart.finance.yahoo.com/table.csv?s=" + ticker)
        let rows = stockInfo.Rows
        let currentRows= rows |> Seq.take(10)
        let x = currentRows |> Seq.map(fun si -> si.Date.ToOADate()) |> Seq.toArray
        let y = currentRows |> Seq.map(fun si -> (float)si.``Adj Close``) |> Seq.toArray
        let regression = SimpleLinearRegression()
        let sse = regression.Regress(x,y)
        let targetDate' = [|targetDate.ToOADate()|]
        regression.Compute(targetDate')

    member public this.GetTicker (phrase:string) =
        let client = new HttpClient()
        let appId = "4015e314-326d-4443-a3ad-854f37352e3d"
        let subscriptionKey = "847c177472014d77924e14b82600f35e"
        let queryString = System.Web.HttpUtility.UrlEncode(phrase)
        let uri = "https://api.projectoxford.ai/luis/v1/application?id=" + appId + "&subscription-key=" + subscriptionKey + "&q=" + queryString
        let message = client.GetAsync(uri).Result
        let response = message.Content.ReadAsStringAsync().Result
        let luis = LuisContext.Parse(response)
        let entity = luis.Entities |> Seq.head
        entity.Entity
