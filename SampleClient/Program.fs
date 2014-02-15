open HadoopFs.Core
open HadoopFs.IO
open HadoopFs.Samples
open HadoopFs.Testability

let inputFile = @"outlineofscience.txt"

//// A reducer exe
//let mainReduce argv =
//    doReduce <| SingleOutput WordCount.Reducer
//    0

[<EntryPoint>]
let mainMap argv =
    doMapReduce <| ManyOutputs WordCount.Mapper <| SingleOutput WordCount.Reducer <| Readers.TestableConsole <| Writers.Console
    0