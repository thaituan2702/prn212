using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Modelss;

namespace VehicleEmissionManagement.Core.Servicess
{
    public class InspectorService : IInspectorService
    {
        private readonly IInspectionRepository _inspectionRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        public InspectorService(
            IInspectionRepository inspectionRepository,
            IVehicleRepository vehicleRepository,
            IAppointmentRepository appointmentRepository)
        {
            _inspectionRepository = inspectionRepository;
            _vehicleRepository = vehicleRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<List<InspectionRecord>> GetPendingInspectionsAsync(int inspectorId)
        {
            // Lấy danh sách các xe cần kiểm định đang chờ
            return await _inspectionRepository.GetPendingInspectionsAsync(inspectorId);
        }

        public async Task<List<InspectionRecord>> GetCompletedInspectionsAsync(int inspectorId, DateTime? startDate = null, DateTime? endDate = null)
        {
            // Mặc định lấy của ngày hôm nay nếu không có ngày bắt đầu và kết thúc
            var start = startDate ?? DateTime.Today;
            var end = endDate ?? DateTime.Today.AddDays(1).AddSeconds(-1);

            // Lấy danh sách các xe đã kiểm định
            var inspections = await _inspectionRepository.GetInspectionsByDateRangeAsync(0, start, end);
            return inspections.Where(i => i.InspectorID == inspectorId).ToList();
        }

        public async Task<bool> SubmitInspectionResultAsync(InspectionRecord inspectionRecord)
        {
            // Đặt các giá trị mặc định nếu chưa có
            inspectionRecord.InspectionDate = DateTime.Now;
            inspectionRecord.CreatedAt = DateTime.Now;
            inspectionRecord.UpdatedAt = DateTime.Now;

            // Tính toán ngày hết hạn (6 tháng sau ngày kiểm định)
            inspectionRecord.ExpiryDate = inspectionRecord.InspectionDate.AddMonths(6);

            // Lưu kết quả kiểm định
            var result = await _inspectionRepository.CreateInspectionAsync(inspectionRecord);

            // Nếu có lịch hẹn tương ứng, cập nhật trạng thái
            if (result)
            {
                var appointments = await _appointmentRepository.GetAppointmentsByStationIdAsync(
                    inspectionRecord.StationID,
                    inspectionRecord.InspectionDate.Date,
                    "Confirmed");

                var matchingAppointment = appointments
                    .FirstOrDefault(a => a.VehicleID == inspectionRecord.VehicleID);

                if (matchingAppointment != null)
                {
                    matchingAppointment.Status = "Completed";
                    matchingAppointment.UpdatedAt = DateTime.Now;
                    await _appointmentRepository.UpdateAppointmentAsync(matchingAppointment);
                }
            }

            return result;
        }

        public async Task<List<InspectionRecord>> SearchInspectionsAsync(int inspectorId, string searchTerm)
        {
            // Lấy tất cả các buổi kiểm định của inspector
            var allInspections = await _inspectionRepository.GetInspectionsByStationIdAsync(0);
            var inspectorInspections = allInspections.Where(i => i.InspectorID == inspectorId).ToList();

            if (string.IsNullOrWhiteSpace(searchTerm))
                return inspectorInspections;

            // Tìm kiếm theo biển số hoặc tên chủ xe
            searchTerm = searchTerm.ToLower();
            return inspectorInspections.Where(i =>
                (i.Vehicle?.PlateNumber?.ToLower().Contains(searchTerm) ?? false) ||
                (i.Vehicle?.Owner?.FullName?.ToLower().Contains(searchTerm) ?? false)
            ).ToList();
        }

        public async Task<Vehicle> GetVehicleDetailsAsync(int vehicleId)
        {
            return await _vehicleRepository.GetVehicleByIdAsync(vehicleId);
        }

        public async Task<List<Appointment>> GetTodayAppointmentsAsync(int stationId)
        {
            return await _appointmentRepository.GetAppointmentsByStationIdAsync(stationId, DateTime.Today, "Confirmed");
        }
    }
}