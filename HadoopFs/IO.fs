namespace HadoopFs.IO

open System
open System.IO

/// Provides a set of readers to use within streaming map tasks.
module Readers = 
    /// Reads from the Console as a sequence of strings.
    let Console =
        seq { 
            let line = ref (Console.ReadLine())
            while (!line <> null) do
                yield !line
                line := Console.ReadLine() 
        }
    
    /// Reads from a file.
    let FileSystem(path : string) = File.ReadLines(path)

/// Provides a set of writers to use within streaming reduce tasks.
module Writers = 
    /// Writes to the Console
    let Console (line:string) = Console.WriteLine line
    
    /// Writes to a file
    let FileSystem path = 
        fun (line : string) -> 
            use writer = new StreamWriter(path, true)
            writer.WriteLine line
