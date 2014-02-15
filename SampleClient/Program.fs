open HadoopFs.Core
open HadoopFs.Samples

let inputFile = @"outlineofscience.txt"

// A reducer exe
//let main argv =
//    doReduce WordCount.Reducer
//    0

[<EntryPoint>]
let main argv = 
    doMap WordCount.Mapper
    0
