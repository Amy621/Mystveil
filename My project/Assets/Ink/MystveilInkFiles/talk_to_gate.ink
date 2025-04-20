-> talk_to_gate

== talk_to_gate ==
{ not empathy and not courage and not justice:
    Liora: Huh, it looks like there's three objects missing. If I can find them maybe the gate will open!
}
{ empathy or courage or justice:
    Liora: Which one should I find next?
}
    + {not empathy} [Find the yellow gem]
        -> empathy
    + {not justice} [Find the blue gem]
        -> justice
    + {not courage} [Find the red gem]
        -> courage
