#!/usr/bin/env python3
# -*- coding: utf-8 -*-

from random import randrange
import io
from random import randrange

path_source_file = "/Users/dustin/Downloads/NPCs.csv"

path_output_1 = "/Users/dustin/Downloads/NPCs_1.csv"
path_output_2 = "/Users/dustin/Downloads/NPCs_2.csv"
path_output_3 = "/Users/dustin/Downloads/NPCs_3.csv"


def w(x, y):
    with open(x, "a") as x:
        x.write(y)


count = 0
with open(path_source_file, "r") as source_file:
    for line in source_file:
        z = randrange(7)  # all possible combos
        if z == 0:
            w(path_output_1, line)
        elif z == 1:
            w(path_output_2, line)
        elif z == 2:
            w(path_output_2, line)
        elif z == 3:
            w(path_output_1, line)
            w(path_output_2, line)
        elif z == 4:
            w(path_output_2, line)
            w(path_output_3, line)
        elif z == 5:
            w(path_output_1, line)
            w(path_output_3, line)
        else:
            w(path_output_1, line)
            w(path_output_2, line)
            w(path_output_3, line)
        count = count + 1
        print(count)
