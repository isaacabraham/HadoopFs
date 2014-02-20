/// A sample map reducer that counts words in a document.
module HadoopFs.Samples.WordCount

open System

/// A sample mapper that splits lines based on spaces into words and counts the number of occurences within a line.
let Mapper (row : string) = 
    row.Split([| ' ' |], StringSplitOptions.RemoveEmptyEntries)
    |> Seq.countBy id

/// A sample reducer that counts words supplied from the word count Mapper.
let Reducer (key, values) = 
    let total = 
        values
        |> Seq.map Int32.Parse
        |> Seq.sum
    Some (key, total)