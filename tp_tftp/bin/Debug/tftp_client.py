__author__ = 'aurora'

import ipaddress
from request import Request
from tkinter import *
from tkinter.messagebox import showerror
from tkinter.filedialog import asksaveasfilename, askopenfilename

class InterfaceClient(Tk):

    def __init__(self):
        self.root = None
        Tk.__init__(self, self.root)
        self._initialize()

    def _initialize(self):

        Label(self.root, text="Ip serveur").grid(row=0, column=0, padx=20)
        self.txtIp = Entry(self.root, width=20)
        self.txtIp.insert(0, "127.0.0.1")
        self.txtIp.grid(row=0, column=1)

        BtnWrite = Button(self.root, text="Write")
        BtnWrite.grid(row=1, column=0)
        BtnWrite.bind("<Button-1>", self._btnWriteClick)

        BtnRead = Button(self.root, text="Read")
        BtnRead.grid(row=1, column=1)
        BtnRead.bind("<Button-1>", self._btnReadClick)

    def _validIp(self):
        try:
            ipaddress.IPv4Address(self.txtIp.get())
        except ipaddress.AddressValueError:
            showerror(title="Oups", message="Adresse Ip invalide")
        else:
            return True

    def _btnWriteClick(self, event):

        if self._validIp():
            self.path = askopenfilename(filetypes=[('All', '.*')], title="Write---->Server")
            Request().Put(self.path, self.txtIp.get())

    def _btnReadClick(self, event):

        if self._validIp():
            self.path = asksaveasfilename(filetypes=[('All', '.*')], title="Read<----Server")
            Request().Get(self.path, self.txtIp.get())

if __name__ == "__main__":
    app = InterfaceClient()
    app.title('Client TFTP')
    app.mainloop()
k(self, event):

        if self._validIp():
            self.path = askopenfilename(filetypes=[('All', '.*')], title="Write---->Server")
            Request().Put(self.path, self.txtIp.get())

    def _btnReadClick(self, event):

        if self._validIp():
            self.path = asksaveasfilename(filetypes=[('All', '.*')], title="Read<----Server")
            Request().Get(self.path, self.txtIp.get())

i