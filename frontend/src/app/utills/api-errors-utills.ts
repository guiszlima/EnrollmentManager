import { HttpErrorResponse } from '@angular/common/http';

interface ErrorLike {
  error?: unknown;
  status?: number;
}

export function extractApiErrors(
  err: unknown,
  fallback = 'Não foi possível concluir a operação.'
): string[] {
  const httpError: ErrorLike = err instanceof HttpErrorResponse
    ? err
    : (err as ErrorLike);
  const body = err instanceof HttpErrorResponse
    ? err.error
    : httpError?.error ?? err;

  if (body && typeof body === 'object') {
    const apiResponse = body as { errors?: unknown; message?: unknown; error?: unknown };
    if (Array.isArray(apiResponse.errors)) {
      const errors = apiResponse.errors.filter(
        (value): value is string => typeof value === 'string' && value.trim().length > 0
      );
      if (errors.length > 0) return errors;
    }
    if (apiResponse.errors && typeof apiResponse.errors === 'object') {
      const validationErrors = Object.values(apiResponse.errors as Record<string, unknown>)
        .flatMap(value => Array.isArray(value) ? value : [value])
        .filter((value): value is string => typeof value === 'string' && value.trim().length > 0);
      if (validationErrors.length > 0) return validationErrors;
    }
    if (apiResponse.error && typeof apiResponse.error === 'object') {
      const nested = extractApiErrors({ error: apiResponse.error }, fallback);
      if (nested.length > 0 && nested[0] !== fallback) return nested;
    }
    if (typeof apiResponse.message === 'string' && apiResponse.message.trim().length > 0) {
      return [apiResponse.message];
    }
    const problemDetails = body as { title?: unknown; detail?: unknown };
    if (typeof problemDetails.detail === 'string' && problemDetails.detail.trim().length > 0) {
      return [problemDetails.detail];
    }
    if (typeof problemDetails.title === 'string' && problemDetails.title.trim().length > 0) {
      return [problemDetails.title];
    }
  }

  if (typeof httpError?.status === 'number' && httpError.status !== 0) {
    const statusMessages: Record<number, string> = {
      400: 'Não foi possível validar os dados enviados.',
      401: 'Sua sessão expirou. Entre novamente para continuar.',
      403: 'Você não tem permissão para realizar esta ação.',
      404: 'O recurso solicitado não foi encontrado.',
      409: 'A operação entrou em conflito com um registro existente.',
      500: 'O servidor encontrou um problema. Tente novamente mais tarde.'
    };
    return [statusMessages[httpError.status] ?? `Erro no servidor (${httpError.status}). Tente novamente mais tarde.`];
  }

  if (httpError?.status === 0) {
    return ['Não foi possível conectar ao servidor. Verifique sua conexão.'];
  }

  return [fallback];
}
