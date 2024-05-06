namespace DungFs

type Person = {
  health: int
  gold: int
} with 
  static member empty = { health = 100; gold = 0}
  member this.isDead = this.health <= 0
