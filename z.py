



import pyttsx3 as tts 



engine = tts.init()

def Speak(word):
    
    engine.say(word)
    engine.runAndWait()



while True:
    words = str(input("Enter words : "))
    Speak(words)