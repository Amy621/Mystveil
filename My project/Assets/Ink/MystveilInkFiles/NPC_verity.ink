-> verity_intro

== verity_intro ==
Liora: Hello Ms. Verity!
Verity: Oh hello Liora! Is there anything ailing you today?
    + [Say that you are perfectly healthy]
        -> healthy
    + [Ask if she is doing well]
        -> are_you_healthy
    + [Say that you are unwell]
        -> unwell
        
== healthy ==
Liora: I'm perfectly healthy and going strong today!
Verity: That's good to hear. Be careful when venturing into the forest, I heard there are a lot more dangerous magical creatures lurking recently.
Liora: I'll be careful!
-> END

== are_you_healthy ==
Liora: Are you doing okay Ms. Verity?
Verity: Well, there are more and more people getting hurt from the monsters. I can only do so much with my weak healing magic.
Liora: But your antidotes and potions that you make are really good and effective!
Verity: I don't make them. I have to get them commissioned from an extremely powerful witch. I'm glad they work well, though!
Liora: Huh, who makes them?
Verity: I can't say. They do reside in Mystveil and you know them quite well!
-> END

== unwell ==
Liora: I am not feeling well :(
Verity: Hmm.. You look alright to me so is it somthing bothering you?
    + [Yes]
        -> something_on_my_mind
    + [JK I lied]
        -> lied_oops

== something_on_my_mind ==
Liora: Just worried about getting the gate open.
Verity: Oh... I'm sure everything will be resolved soon! If not you then a famous witch will surely break the spell.
Liora: You're right. Thank you for talking with me, Ms. Verity!
Verity: Anytime.
-> END

== lied_oops ==
Liora: Just kidding!
Verity: That's good. If you were truly unwell, I would be quite worried. Go and have fun :)
Liora: Thank you! Will do o7
-> END