/* eslint-disable @typescript-eslint/no-explicit-any */
export interface ApiError {
  message: string
  statusCode?: number
}

export const handleApiError = (error: any): ApiError => {
  if (error.response) {
    // Server responded with error
    return {
      message: error.response.data.message || 'An error occurred',
      statusCode: error.response.status,
    }
  } else if (error.request) {
    // Request made but no response
    return {
      message: 'No response from server',
      statusCode: 503,
    }
  } else {
    // Request setup error
    return {
      message: error.message || 'Error setting up request',
      statusCode: 400,
    }
  }
}
