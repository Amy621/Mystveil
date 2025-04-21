-> hilbert_intro

== hilbert_intro ==
{ defend_orphans:
    Liora: Hey, Hilbert!
    Hilbert: Hello! Wanna be a part of an Operation??
        + [Ask what the operation is]
            -> ask_about_operation
        + [Ask where Nate is]
            -> ask_about_nate
}
{ not justice:
    Liora: Who is that?
    ???: ... *runs away*
    Liora: Huh, he's gone.
}
{ go_to_friends_side:
    Liora: Hilbert!
    Hilbert: ... *runs away*
    Liora: ...
}
-> END

== ask_about_operation ==
Liora: What's the operation this time?
Hilbert: Operation: Become a Battler!
Liora: Oh? You want to learn how to battle?
Hilbert: Yeah! I'm going to become an awesome Rogue one day and slay my enemies- the Witches!
Liora: ... That's... good?
Hilbert: But not Liora! You're the Good Witch!
Liora: Thanks I guess?
->END

== ask_about_nate ==
Liora: Where's Nate?
Hilbert: Over there! He likes hanging out in town even if everyone dislikes us. He takes the blame for everything we do... so I feel bad but I like pranks.
Liora: Maybe you should do something for him to show that you appreciate him.
Hilbert: You're right! Operation: Cheer Up and Make Nate Happy is a GO!
-> END