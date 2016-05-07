﻿
//install-package FSharp.Data
#r "../packages/FSharp.Data.2.3.0/lib/net40/FSharp.Data.dll"
open FSharp.Data

type Context = CsvProvider<"http://ichart.finance.yahoo.com/table.csv?s=MSFT">

let getStockClose ticker =
    try
        let stockInfo = Context.Load("http://ichart.finance.yahoo.com/table.csv?s=" + ticker)
        let mostRecent = stockInfo.Rows |> Seq.head
        Some mostRecent.``Adj Close``
    with 
        | :?  System.Net.WebException -> None

getStockClose "MSFT"
getStockClose "???"

//install-package FSharp.Charting
#load "../packages/FSharp.Charting.0.90.14/FSharp.Charting.fsx"
open System
open FSharp.Charting

let rows = Context.Load("http://ichart.finance.yahoo.com/table.csv?s=MSFT").Rows
rows 
|> Seq.map(fun si -> si.Date, si.``Adj Close``)
|> Chart.FastLine

//install-package Accord.Statistics
#r "../packages/Accord.3.0.2/lib/net40/Accord.dll"
#r "../packages/Accord.Statistics.3.0.2/lib/net40/Accord.Statistics.dll"
#r "../packages/Accord.Math.3.0.2/lib/net40/Accord.Math.dll"
open Accord
open Accord.Statistics
open Accord.Statistics.Models.Regression.Linear

//let x = rows |> Seq.map(fun si -> si.Date.ToOADate()) |> Seq.toArray
let currentRows= rows |> Seq.take(10)
let x = currentRows |> Seq.map(fun si -> si.Date.ToOADate()) |> Seq.toArray
let y = currentRows |> Seq.map(fun si -> (float)si.``Adj Close``) |> Seq.toArray
let regression = SimpleLinearRegression()
let sse = regression.Regress(x,y)
let mse = sse/float x.Length 
let rmse = sqrt(mse)
let r2 = regression.CoefficientOfDetermination(x,y)

let tomorrow = [|(new DateTime(2016,5,19)).ToOADate()|]
let predict =  regression.Compute(tomorrow)

