import cv2
import numpy as np
import cv2 as cv
"""
cap = cv2.VideoCapture(0)

coords = []
while(1):

    # Take each frame
    _, frame = cap.read()

    # Convert BGR to HSV

    hsv = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)

    # define range of blue color in HSV 
    # lower_red = np.array([160,140,50])
    # upper_red = np.array([180,255,255])
    # Green

    green = np.uint8([[[38, 87, 88]]])
    hsv_green = cv2.cvtColor(green,cv2.COLOR_RGB2HSV)

    lower_red = np.array([ hsv_green[0][0][0]-10, 100, 100])
    upper_red = np.array([ hsv_green[0][0][0]+10, 255, 255])

    imgThreshHigh = cv2.inRange(hsv, lower_red, upper_red)
    thresh = imgThreshHigh.copy()

    countours,_ = cv2.findContours(thresh, cv2.RETR_LIST,cv2.CHAIN_APPROX_SIMPLE)

    if countours:
        max_area = 1
        best_cnt = None
        for cnt in countours:
            area = cv2.contourArea(cnt)
            if area > max_area:
                max_area = area
                best_cnt = cnt

        # If we found a big enought circle
        if not best_cnt is None:
            M = cv2.moments(best_cnt)
            cx,cy = int(M['m10']/M['m00']), int(M['m01']/M['m00'])
            coord = cx, cy #This are your coordinates for the circle
            # area = moments['m00'] save the object area
            #perimeter = cv2.arcLength(best_cnt,True) is the object perimeter

            #Save the coords every frame on a list
            #Here you can make more conditions if you don't want repeated coordinates
            # points.append(coord)
    else:
        print('No cirle found')

    cv2.imshow('frame',frame)
    cv2.imshow('Object',thresh)
    k = cv2.waitKey(5) & 0xFF
    if k == 27:
        break
"""
def find_circle(frame):

    # Convert BGR to HSV
    hsv = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)

    # define range of blue color in HSV 
    # lower_red = np.array([160,140,50])
    # upper_red = np.array([180,255,255])
    # Green

    green = np.uint8([[[38, 87, 88]]])
    hsv_green = cv2.cvtColor(green,cv2.COLOR_RGB2HSV)

    print(hsv_green[0][0][0])

    lower_red = np.array([ hsv_green[0][0][0]-10, 100, 100])
    upper_red = np.array([ hsv_green[0][0][0]+10, 255, 255])

    imgThreshHigh = cv2.inRange(hsv, lower_red, upper_red)
    thresh = imgThreshHigh.copy()

    cv2.imshow("Thresh", thresh)

    countours,_ = cv2.findContours(thresh, cv2.RETR_LIST,cv2.CHAIN_APPROX_SIMPLE)

    if countours:
        max_area = 1
        best_cnt = None
        for cnt in countours:
            area = cv2.contourArea(cnt)
            if area > max_area:
                max_area = area
                best_cnt = cnt

        # If we found a big enought circle
        if not best_cnt is None:
            M = cv2.moments(best_cnt)
            cx,cy = int(M['m10']/M['m00']), int(M['m01']/M['m00'])
            coord = cx, cy #This ar e your coordinates for the circle
            return coord
            # area = moments['m00'] save the object area
            #perimeter = cv2.arcLength(best_cnt,True) is the object perimeter

            #Save the coords every frame on a list
            #Here you can make more conditions if you don't want repeated coordinates
            # points.append(coord)

    return None