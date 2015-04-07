__author__ = 'aurora'

import socket
import os
import trame
from threading import Thread


class Request(object):

    def __init__(self):
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

    def Get(self, fichier, ipaddr):
        fichier = os.path.basename(fichier)
        boolretour = False
        for i in range(3):
            self.sock.sendto(trame.RRQ(fichier), (ipaddr, 69))
            retour = self.sock.recvfrom(516)
            if trame.is_DATA(retour[0][:4], 1):
                boolretour = True
                break
            elif trame.is_error(retour[0]):
                print(str(retour[0][4:]))
                break
        if boolretour:
            ReadRequest(fichier, retour[0][4:], self.sock.dup(), retour[1])
        else:
            print("Connection with server failed")

    def Put(self, fichier, ipaddr):
        fichier = os.path.basename(fichier)
        boolretour = False
        for i in range(3):
            self.sock.sendto(trame.WRQ(fichier), (ipaddr, 69))
            retour = self.sock.recvfrom(50)
            if trame.is_ACK(0, retour[0]):
                boolretour = True
                break
            elif trame.is_error(retour[0]):
                print(str(retour[0][4:]))
                break
        if boolretour:
            WriteRequest(fichier, retour[1], self.sock.dup())
        else:
            print("Connection with server failed")


class WriteRequest(Thread):

    def __init__(self, fichier, ipaddr, socket):

        self.s = socket
        self.ip = ipaddr
        self.fichier = fichier

        Thread.__init__(self, name="Thread Write Request", target=self.run)
        self.start()

    def run(self):
        #Variable bloc
        bloc = 1
        retry = 0
        bThread = False
        bufferdata = None
        retourdata = None
        #Creation de l'object socket utilise
        f = open(self.fichier, "rb")

        bThread = True

        while bThread:
            f.seek((bloc-1)*512)
            bufferdata = f.read(512)
            self.s.sendto(trame.DATA(bloc, bufferdata), self.ip)


            retourdata = self.s.recv(50)

            if trame.is_ACK(bloc, retourdata):
                retry = 0
                bloc += 1
                if len(bufferdata) < 512:
                    bThread = False
                    print("Fin du transfert")
            elif trame.is_error(retourdata[:2]):
                print("Erreur X")
                bThread = False
            else:
                retry += 1

            if retry == 3:
                bThread = False

        f.close()


class ReadRequest(Thread):

    def __init__(self, fichier, firstbloc, socket, ip):

        self.ip = ip
        self.s = socket
        self.fichier = fichier
        self.firstbloc = firstbloc

        Thread.__init__(self, name="THread REad", target=self.run)
        self.start()

    def run(self):

        f = open(self.fichier, 'wb')
        f.write(self.firstbloc)

        retourData = None
        bThread = True
        bloc = 1
        retry = 0

        while bThread:
            self.s.sendto(trame.ACK(bloc), self.ip)

            retourData = self.s.recv(516)
            if bloc == 65535:
                bloc = 0
            if trame.is_DATA(retourData[:4], bloc+1):
                bloc += 1
                retry = 0
                f.write(retourData[4:])

                #Fin de la requete de lecture dernire paquet
                if len(retourData[4:]) < 512:
                    bThread = False
                    self.s.sendto(trame.ACK(bloc), self.ip)
                    print("Fin du transfert")

            elif trame.is_error(retourData[:2]):
                bThread = False
                retry = 3
                print(str(retourData[4:]))
            else:
                retry += 1

            if retry == 3:
                f.close()
                os.remove(self.fichier)
                bThread = False
                print("Conenction with server stop")
        f.close()







False
                    self.s.sendto(trame.ACK(bloc