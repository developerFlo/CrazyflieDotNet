/* 
 *						 _ _  _     
 *		       ____ ___  ___  __________(_|_)(_)____
 *		      / __ `__ \/ _ \/ ___/ ___/ / _ \/ ___/
 *		     / / / / / /  __(__  |__  ) /  __/ /    
 *		    /_/ /_/ /_/\___/____/____/_/\___/_/  
 *
 *	     Copyright 2013 - Messier/Chris Karcz - ckarcz@gmail.com
 *
 *	This Source Code Form is subject to the terms of the Mozilla Public
 *	License, v. 2.0. If a copy of the MPL was not distributed with this
 *	file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

namespace CrazyflieDotNet.Crazyflie.TransferProtocol
{
	public class PingPacketHeader
		: OutputPacketHeader, IPingPacketHeader
	{
		public PingPacketHeader(byte headerByte)
			: base(headerByte)
		{
		}

		public PingPacketHeader(Channel channel = DefaultChannel)
			: this(Port.All, channel)
		{
		}

		public PingPacketHeader(Port port, Channel channel = DefaultChannel)
			: base(port, channel)
		{
		}
	}
}