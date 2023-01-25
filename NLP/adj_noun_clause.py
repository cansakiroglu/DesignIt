import spacy

# %%
nlp = spacy.load(r'C:\\Users\\BASAK\\AppData\\Local\\Programs\\Python\\Python39\\Lib\\site-packages\\en_core_web_sm-3.0.0\\en_core_web_sm\\en_core_web_sm-3.0.0',disable=['ner','textcat'])

# %%
from spacy.matcher import Matcher 

from spacy import displacy 
import visualise_spacy_tree
from IPython.display import Image, display

# %%
def find_adj_noun(text):
    
    doc = nlp(text)

    pat = []
    
    for token in doc:
        phrase = ''
        if (token.pos_ == 'NOUN')\
            and (token.dep_ in ['dobj','pobj','nsubj','nsubjpass']):

            for subtoken in token.children:
                if (subtoken.pos_ == 'ADJ') or (subtoken.dep_ == 'compound'):
                    phrase += subtoken.text + ' '

            if len(phrase)!=0:
                phrase += token.text
             
        if  len(phrase)!=0:
            pat.append(phrase)
        
    
    return pat

# %%
sent = "I desire red pencil."
output = find_adj_noun(sent)

# %%
output

# %%





