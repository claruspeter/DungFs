namespace DungFs

type Dice = {
  ///<summary>The function to produce a value from 1 up to the provided maximum, inclusive</summary>
  rollFunction: int -> int
}with

  member this.roll maxValue =
    this.rollFunction maxValue
    |> max 1
    |> min maxValue
