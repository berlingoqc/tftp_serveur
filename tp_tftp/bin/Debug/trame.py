__author__ = 'admin'

import struct

def RRQ(file):
    print("Read Request for " + file)
    return b'\x00\x01' + str.encode(file) + b'\x00' + str.encode("octect") + b'\x00'

def WRQ(file):
    print("Write Request for " + file)
    return b'\x00\x02' + str.encode(file) + b'\x00' + str.encode("octect") + b'\x00'

def DATA(bloc, data):
    print("Sending DATA #" + str(bloc))
    return b'\x00\x03' + struct.pack(">H", bloc) + data

def ACK(bloc):
    print("Sending ACK #" + str(bloc))
    return b'\x00\x04' + struct.pack(">H", bloc)

#Retour vrai si la trame est un ack avec le bon bloc
def is_ACK(block, data):
    if data == ACK(block):
        print("Reception ACK #" + str(block))
        return True
    else:
        return False


def is_error(data):
    if data[:2] == b'\x00\x05':
        return True
    else:
        return False


#Retour vrai et les data de la trame si trame valide sinon retour faux
def is_DATA(data, block):
    if data == b'\x00\x03' + struct.pack(">H", block):
        print("Reception DATA #" + str(block))
        return True
    else:
        return False


loc
def is_ACK(block, data):
    if data == ACK(block):
        print("Reception ACK #" + str(block))
        return True
    else:
        return False


def is_error(data):
    if data[:2] == b'\x00\x05':
        return True
    else:
        return False


#Retour vrai et les data de la trame si trame valide sinon retour faux
def is_DATA(data, block):
    if data == b'\x00\x03' + struct.pack(">H", block):
        print("Reception