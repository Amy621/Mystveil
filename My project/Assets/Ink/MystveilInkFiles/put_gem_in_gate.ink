-> put_gem_in_gate

== put_gem_in_gate ==
{ empathy or courage or justice:
    Liora: Fits in perfectly! One down, two more to go!
    -> talk_to_gate
}
{ (empathy and courage) or (empathy and justice) or (justice and courage):
    Liora: That's two! One more to go.
    -> talk_to_gate
}
{ empathy and courage and justice:
    Liora: That's all of them! I did it!
    
    { player_charisma < 5:
        Liora: Now I can finally leave this town! Goodbye and good riddance!
    }
    { player_charisma >= 5:
        Liora: Thank you Mystveil, I'll never forget these memories.
    }
    -> END
}
-> END