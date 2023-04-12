import string
class UnderstandCommand:
	def __init__(self,val):
		self.val = val
	
	def get_command(self):
		video_chat = ["video chat", "live stream", "video call", "call"] # label: 0
		goruntu = ["image", "view", "video"] # label: 1
		ses = ["sound", "voice"] # label: 2
		envanter = ["inventory", "items", "list of goods","in game menu", "menu for game","menu for the game"] # label: 3
		#cagri = ["call"] # video chate gonderildi
		menu = ["menu", "main menu"] #label 11
		olumsuz = ["close", "turn off", "end", "shut down"] #kapa sonlandir #0
		olumlu = ["open", "turn on", "start"] #1
		olumlu_partial = ["turn", "on"] #1
		olumsuz_partial = ["turn", "off"] #0
		menu_ozel = ["back to", "turn back", "return"] #11
		cmnd = self.val
		label = []
		sentence = cmnd.lower()
		sentence = sentence.translate(str.maketrans('', '', string.punctuation))

		if any(item in sentence for item in video_chat):
			label.append(0)
			if any(status in sentence for status in olumlu):
				label.append(1)
				return label
			elif all(status in sentence for status in olumlu_partial):
				label.append(1)
				return label
			elif all(status in sentence for status in olumsuz_partial):
				label.append(0)
				return label
			elif any(status in sentence for status in olumsuz):
				label.append(0)
				return label
			else:
				label.append(12)
				return label
		
		elif any(item in sentence for item in goruntu):
			label.append(1)
			if any(status in sentence for status in olumlu):
				label.append(1)
				return label
			elif all(status in sentence for status in olumlu_partial):
				label.append(1)
				return label
			elif all(status in sentence for status in olumsuz_partial):
				label.append(0)
				return label
			elif any(status in sentence for status in olumsuz):
				label.append(0)
				return label
			else:
				label.append(12)
				return label
			
		elif any(item in sentence for item in ses):
			label.append(2)
			if any(status in sentence for status in olumlu):
				label.append(1)
				return label
			elif all(status in sentence for status in olumlu_partial):
				label.append(1)
				return label
			elif all(status in sentence for status in olumsuz_partial):
				label.append(0)
				return label
			elif any(status in sentence for status in olumsuz):
				label.append(0)
				return label
			else:
				label.append(12)
				return label

		elif any(item in sentence for item in envanter):
			label.append(3)
			if any(status in sentence for status in olumlu):
				label.append(1)
				return label
			elif all(status in sentence for status in olumlu_partial):
				label.append(1)
				return label
			elif all(status in sentence for status in olumsuz_partial):
				label.append(0)
				return label
			elif any(status in sentence for status in olumsuz):
				label.append(0)
				return label
			else:
				label.append(12)
				return label	

		elif any(item in sentence for item in menu):
			label.append(11)
			if any(status in sentence for status in menu_ozel):
				label.append(11)
				return label
			else:
				label.append(12)
				return label
		else:
			label.append(12)
			label.append(12)
			return label			
