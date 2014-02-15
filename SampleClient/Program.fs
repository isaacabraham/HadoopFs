open HadoopFs.Core
open HadoopFs.Samples

let inputFile = @"outlineofscience.txt"

//// A reducer exe
//let mainReduce argv =
//    doReduce <| SingleOutput WordCount.Reducer
//    0

[<EntryPoint>]
let mainMap argv =
    HadoopFs.Testability.doMapReduce <| ManyOutputs WordCount.Mapper <| SingleOutput WordCount.Reducer <| HadoopFs.IO.Readers.TestableConsole <| HadoopFs.IO.Writers.Console
    0