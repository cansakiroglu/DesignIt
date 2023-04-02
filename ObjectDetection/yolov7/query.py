from detect import detect

img = 'inference/images/1.png' # Will be taken as a parameter, here inference/images/1.png is an example.

pred = detect(img)[0]

dictionary = {
    56: 'chair',
    57: 'couch',
    58: 'potted plant',
    59: 'bed',
    60: 'dining table',
    61: 'toilet',
    62: 'tv',
    63: 'laptop',
    68: 'microwave',
    69: 'oven',
    71: 'sink',
    72: 'refrigerator',
    74: 'clock',
    75: 'vase'
}

if len(pred) == 0:
    print('unknown')
    print('(No Prediction!)')
else:
    max_conf = 0
    max_conf_i = -1
    for i in range(len(pred)):
        if pred[i][4] > max_conf:
            max_conf = pred[i][4]
            max_conf_i = i
    
    print(dictionary[int(pred[max_conf_i][5])])
