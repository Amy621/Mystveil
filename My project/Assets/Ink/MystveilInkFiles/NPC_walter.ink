-> walter_intro

== walter_intro ==
Walter: If it isn't Little Liora! What's up?
    + [Ask about the gemstones]
        -> have_seen_gems
    + [Ask about the family]
        -> ask_about_family
    + [Ask about the town]
        -> ask_about_town
        
== have_seen_gems ==
{ empathy and courage and justice:
    Liora: I found all the gems!
    Walter: Thank you so much, Liora! Now the trade routes and everything are back up! You've brought the town back to life!
    Liora: Of course, Mr. Walter. Glad to help!
}

{ not empathy or not courage or not justice:
    Liora: Have you seen the gems?
    Walter: Not since they flew out a while ago.
    Liora: Well, it was worth asking.
    Walter: Good luck on your search, youngster!
}
-> END

== ask_about_family ==
Liora: How's Sally?
Walter: She's doing fine. She's getting a bit heavier than average so I'm afraid she might not be able to fly anymore.
Liora: That's unfortunate. I remember when I was little and Sally would fly us around Mystveil. She was always super happy to eat the apples.
Walter: Maybe that's why that pegasus was gaining weight...
-> END

== ask_about_town ==
Liora: How's the town? As the mayor, I'm sure there's a lot to do and keep track of.
Walter: It sure is a busy job, but I love all the people here and want to help everyone thrive.
{ not empathy or not courage or not justice :
    Walter: Since the gate is closed, the trade routes haven't been opened. I'm afraid we might run out of items soon.
    Liora: That's terrible! I got to open the gate soon.
}
{ empathy and courage and justice :
    Walter: Since the gate has opened, the trade routes can continue and we finally have money flowing back into the town! Thank you again for all your hard work.
    Liora: No problem! Glad to help!
}
-> END