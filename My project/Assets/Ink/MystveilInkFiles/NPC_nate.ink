-> nate_intro

== nate_intro ==
{ defend_orphans:
    Liora: Hey Nate!
    Nate: Liora. Nice to see you again.
        + [Ask about how the orphanage and everyone is doing]
            -> hows_the_orphanage
        + [Ask about his plans for the future]
            -> what_will_you_be_doing
}
{ not justice:
    ???: Excuse me.
    Liora: Sorry about that.
    ???: No worries.
    Liora: Who was that?
}
{ go_to_friends_side:
    Liora: Nate!
    Nate: ... Goodbye Liora.
    Liora: Wait... He's gone.
}
-> END

== hows_the_orphanage ==
Liora: How's the orphanage and everyone? You still the eldest brother?
Nate: Yeah, being the eldest comes with a lot of responsibilities but I love taking care of everyone.
Liora: That sounds stressful but also rewarding. I'm an only child so I don't really get it.
Nate: Haha, being a twin can be challenging but I also have someone to talk to about anything really.
Liora: How's Hilbert?
Nate: He's doing better after you sided with us that day. I think he has some crazy dream of his own now.
    + [Ask about his dream]
        -> what_will_you_be_doing
    + [Ask about Riley]
        -> riley_doing_ok

== what_will_you_be_doing ==
Liora: What about you? Have any plans for the future?
Nate: Hmm... I guess just continuing to take care of everyone in the orphanage. But I'd like to eventually go on a journey and see what's out there.
Liora: That's a good goal. I hope that you can go and explore the world outside of Mystveil.
Nate: Yeah, me too. Thanks for the support.
Liora: Of course.
-> END

== riley_doing_ok ==
Liora: How about Riley?
Nate: He's doing great. He's actually been practicing swordsmanship with a practice wooden sword Thomas made him.
Liora: So Thomas can make him stuff but not me???
Nate: I guess he just wants you to practice the witch's way of making things- crafting.
Liora: *sigh* Good for him, I guess.
-> END