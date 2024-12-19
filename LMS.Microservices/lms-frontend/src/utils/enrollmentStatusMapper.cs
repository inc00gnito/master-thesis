// utils/enrollment-status.ts
import { EnrollmentStatus } from '@/types/enrollment';

class EnrollmentStatusUtils {
  static mapEnrollmentStatus(numericStatus: number): EnrollmentStatus {
    switch (numericStatus) {
      case 0:
        return EnrollmentStatus.Pending;
      case 1:
        return EnrollmentStatus.Active;
      case 2:
        return EnrollmentStatus.Completed;
      case 3:
        return EnrollmentStatus.Cancelled;
      default:
        return EnrollmentStatus.Pending;
    }
  }

  static mapEnrollmentStatusToString(numericStatus: number): string {
    switch (numericStatus) {
      case 0:
        return EnrollmentStatus.Pending;
      case 1:
        return EnrollmentStatus.Active;
      case 2:
        return EnrollmentStatus.Completed;
      case 3:
        return EnrollmentStatus.Cancelled;
      default:
        return EnrollmentStatus.Pending;
    }
  }
}

export default EnrollmentStatusUtils;