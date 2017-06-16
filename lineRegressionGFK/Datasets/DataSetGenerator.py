# Data Set Generator for project

from random import uniform

amountOfPoints = 30
noiseLvl = 7  # random value from (-noiseLvl to noiseLvl) will be added to y_i
bottomX = -100
topX = 100
outputFilename = "dataset.txt"

cooefficientLimit = 5
baseDegree = 1  # a0 + a1x+ a2x^2 + ... abaseDegree*x^baseDegree

cooefficients = tuple(uniform(-1, 1) * cooefficientLimit for _ in range(baseDegree + 1))
cooefficients = (0, 1) # Force cooefficients. Size of tuple enforces baseDegree.

with open("dataset.txt", "w") as file:
    for _ in range(amountOfPoints):
        x = uniform(bottomX, topX)
        y = 0
        for iter, v in enumerate(cooefficients):
            y += x**iter * v
            y += uniform(-noiseLvl, noiseLvl)
        file.write(str(x)+ " " + str(y) + '\n')
