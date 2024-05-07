namespace DungFs

type Person = {
  name: string
  health: int
  gold: int
} with 
  static member empty = { name = "No-one";health = 100; gold = 0}
  member this.isDead = this.health <= 0
