import keras
import pandas as pd
import sys
import numpy

GazeLX = float(sys.argv[1])
GazeLY = float(sys.argv[2])
GazeLZ = float(sys.argv[3])
GazeRX = float(sys.argv[4])
GazeRY = float(sys.argv[5])
GazeRZ = float(sys.argv[6])
PupilL = float(sys.argv[7])
PupilR = float(sys.argv[8])
HR = float(sys.argv[9])
GSR = float(sys.argv[10])

model = keras.models.load_model(r'D:\Projects\NaosQGMouse-DataCollecting\Assets\Scripts\ConationModel.HDF5')
data = [GazeLX, GazeLY, GazeLZ, GazeRX, GazeRY, GazeRZ, PupilL, PupilR, HR, GSR]
data = numpy.array([data])
predictVector = model.predict(data)
prediction = numpy.argmax(predictVector)
print(prediction)

