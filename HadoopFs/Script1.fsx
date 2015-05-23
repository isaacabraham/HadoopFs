
open System

let testString = "hello hello there"

//id is a FSharp operator ->
//it is the same as writing 
//Seq.countBy(fun s -> s)

let result = testString.Split([| ' ' |], StringSplitOptions.RemoveEmptyEntries) |> Seq.countBy id
result

