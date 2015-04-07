__author__ = 'Administrateur'
import trame
import socket
from WriteRequest import *
from time import sleep

class tftp(object):

    def __init__(self, ipaddr):
        self.erreumsg = ["Not defined", "File not found", "Acces violation",
                "Disk full or allocation exceeded", "Illegal TFTP operation",
                "Unknow transfer id", "File already exists","No such user"]
        self.ipaddr = ipaddr
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        pass

    def Get(self, fichier):
        pass

    def Put(self, fichier):
        boolretour = False
        for i in range(1, 3):
            self.sock.sendto(trame.RRQ(fichier), self.ipaddr)
            retour = self.sock.recvfrom(50)
            if trame.is_ACK(0, retour[0]):
                boolretour = True
            elif trame.is_error(retour[0]):
                print(self.erreumsg[int(retour[3], 10)])
            sleep(1)
        if boolretour:
            WriteRequest(fichier, retour[1])









a = tftp(("127.0.0.1", 69))
a.Put("allo.txt")ier):
        boolretour = False
        for i in range(1, 3):
            self.sock.sendto(trame.RRQ(fichier), self.ipaddr)
            retour = self.sock.recvfrom(50)
            if trame.is_ACK(0, retour[0]):
                boolretour = True
            elif trame.is_error(retour[0]):
                print(self.erreumsg[int(retour[3], 10)])
            sleep(1)
        if boolretour:
            WriteRequest(fichier, r