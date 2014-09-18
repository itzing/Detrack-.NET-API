using System;
using System.Collections.Generic;
using System.Drawing;
using Detrack.Model;
using Detrack.Model.Deliveries;

namespace Detrack.Data.Interfaces
{
	public interface IDeliveryRepository
	{
		IEnumerable<Delivery> GetAllForDate(DateTime date);
		Delivery GetDelivery(DateTime deliveryDate, string deliveryDo);
		ViewResponse GetDeliveries(IEnumerable<Delivery> deliveries);

		AddResponse Add(List<Delivery> deliveries);
		EditResponse EditDeliveries(List<Delivery> deliveries);
		DeleteResponse DeleteDeliveriesForDate(DateTime date);

		Image GetSignatureImage(Delivery delivery);
	}
}
