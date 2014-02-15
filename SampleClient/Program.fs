open HadoopFs.Core
open HadoopFs.Samples

let inputFile = @"outlineofscience.txt"

// A reducer exe
let mainReduce argv =
    doReduce <| SingleOutput WordCount.Reducer
    0

[<EntryPoint>]
let mainMap argv =
    doMap <| ManyOutputs WordCount.Mapper
    0