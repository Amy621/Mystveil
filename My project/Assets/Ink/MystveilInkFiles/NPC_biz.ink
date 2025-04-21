-> biz_intro

== biz_intro ==
Liora: Hello Mr. Biz!
Biz: Hey Liora! You want something to eat? Biz's restaurant and bookstore is open!
    + [Ask about his family]
        -> ask_about_biz_family
    + [Leave]
        -> leave_biz
        
== ask_about_biz_family ==
Liora: How's Eliza and Henry?
Biz: Eliza is hard at work trying to get new books for our shelves- as always. She's single-handedly running the bookstore since our other part-timer quit.
Liora: I hope she's not working too hard!
Biz: I'll let her know that you stopped by! As for Henry, he's studying hard. I think he was accepted into the Magic Academia in the fall.
Liora: Congrats to him! That's awesome!
Biz: Thank you! I'll be sad to see him go but I know he wants this.
-> END

== leave_biz ==
Liora: No, I'm good! Thank you.
Biz: Anytime you want a pick me up, just come over to Biz's!
-> END