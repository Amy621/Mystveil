-> myrtle_intro

== myrtle_intro ==
{ defend_orphans:
    Liora: ...
    Myrtle: I can't be associating with you, but you did what was right.
    Liora: Thanks Myrtle. 
}
{ not justice:
    Liora: Hi Myrtle!
    Myrtle: L-Liora. How are you?
        + [Ask what's up]
            -> whats_up_myrtle
        + [Ask about Rose]
            -> whats_up_rose
}
{ go_to_friends_side:
    Liora: Hi Myrtle!
    Myrtle: L-Liora. How are you?
        + [Ask what's up]
            -> whats_up_myrtle
        + [Ask about Rose]
            -> whats_up_rose
}

== whats_up_myrtle ==
Liora: What's up with you, Myrtle?

{ not justice:
    Myrtle: Just keeping track of the funds for the tea party, Rose's schedule, my studying...
    Liora: That's quite a bit. 
    Myrtle: Yeah but it keeps my mind off of the future. W-w-who knows i-if I can be a good enough witch.
    Liora: I'm sure you'll do great! Your paper magic isn't super strong but it can be used in many situations!
    Myrtle: Thank you Liora :)
}

{ go_to_friends_side:
    Myrtle: Packing for going to the Academia.
    Liora: I guess I'll see you if you visit the capital!
    Myrtle: Yeah, see you :)
}
-> END

== whats_up_rose ==
Liora: What's up with Rose?

{ not justice:
    Myrtle: She's buying special tea leaves for our party... that do cost a pretty penny...
    Liora: ... Oh no.
    Myrtle: Yeah... we don't really even drink the tea...
}

{ go_to_friends_side:
    Myrtle: She's packing for the Academia, same as me.
    Liora: Hope that you get there safely.
    Myrtle: Thank you :D
}
-> END
