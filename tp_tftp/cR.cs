using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace tp_tftp
{
    class cR
    {
        private bool _bThread;
        private EndPoint _pd;
        private string _fichier;

        public cR(sInfoTh info)
        {
            _bThread = false;
            _pd = info.pDistant;
            _fichier = info.Nom;
            Thread t = new Thread(new ThreadStart(thR));
            t.Start();
        }

        private void thR()
        {
            byte[] databloc = new byte[512];
            int bLue = 0;
            cTrame trame = new cTrame();
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            FileStream fs;
            EndPoint pl = new IPEndPoint(0, 0);
            s.Bind(pl);
    
            UInt16 bloc = 1;
            int retry = 0;
            byte[] ack = new byte[50];
            //Regarde si le fichier existe
            if (File.Exists(_fichier))
            {
                _bThread = true;
                fs = new FileStream(_fichier, FileMode.Open, FileAccess.Read);
            }
            else
            {
                //S'il n'existe pas mets les data a zero pour sauter la boucle et referme le socket
                //Envoie msg erreur 01
                s.SendTo(trame.Erreur(1), SocketFlags.None, _pd);
                fs = null;
            }

                while ( _bThread)
                {
                    //les un bloc de data de max 512 byte, selon le bloc a envoyer
                    /*Si bloc = 1 skip(0) donc commence au debut et en prend 512
                     *  bloc = 2 skip(512) et en prend 512
                     *  ...
                     *  jusqu'a ce que le skip soit egale a la longeur fichier
                     * */
                    bLue = fs.Read(databloc,0,512);

                    s.SendTo(trame.do_DATA(bloc,databloc.Take(bLue).ToArray()),SocketFlags.None, _pd);

                    s.Poll(3000000,SelectMode.SelectRead);
                    s.Receive(ack);

                    if (trame.is_ACK(bloc, ack))
                    {
                        retry = 0;
                        bloc++;
                        if (bLue < 512)
                        {
                            _bThread = false;
                            fs.Close();
                        }
                    }
                    else
                    {
                        retry++;
                    }

                    if (retry == 3)
                    {
                        s.SendTo(trame.Erreur("Nombre essai trop élever"), SocketFlags.None, _pd);
                        _bThread = false;
                    }

                }
                
            s.Close();
        }

    }
}
