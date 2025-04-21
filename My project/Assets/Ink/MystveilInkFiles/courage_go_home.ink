-> go_home

== go_home ==
Liora: Hecuba! Where are you? 
Liora: Huh, usually he sits out here and greets me. Where could he have gone?
Gale: Heh, you're back!
Liora: Ugh, its Gale...
Gale: Hey! Show some respects to your elders. I've been around since you were just a wee baby.
Liora: I would if you'd stop stealing my stuff and causing a mess everywhere! You're just a little ferret to me.
Gale: Excuse me- I'm a Polecat! Because of that I'm not telling you where Hecuba is. Hmph.
Liora: What?! That's unfair!
    + [Try to negotitate]
        -> negotiate
    + [Attack him! He gets what he deserves]
        -> attack
    + [Ignore him and try to find Hecuba yourself]
        -> ignore

== negotiate ==
Liora: ... If I do one of your chores for you will you tell me?
Gale: Well I might reconsider IF you find the Fairy Flax I'm supposed to give Hecate.
Liora: And now I have to be your messenger too... Fine. I'll find it and tell mom what happened.
Gale: Good luck getting her out of her room.
Liora: ... You're right. I'll still tell Hecuba on you!!
Gale: :P
-> END

== ignore ==
~ player_charisma += 3
Liora: ...
Gale: He's not that way hehe.
Liora: ...
Gale: Not that way either.
Liora: He's kind of getting on my nerves...
    + [Attack him now!]
        -> attack
    + [Negotiate]
        -> negotiate
        
== attack ==
~ player_charisma -= 4
Liora: UGH! I've had enough! I'll defeat you once and for all!
Gale: You can try if you want! You won't beat me!
-> END