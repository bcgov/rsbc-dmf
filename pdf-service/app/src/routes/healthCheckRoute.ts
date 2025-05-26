import { Router, Request, Response } from 'express';

const router = Router();

router.get('/healthcheck', (_req: Request, res: Response) => {
  res.status(200).send({ status: 'OK', message: 'Formio service is healthy.' });
});

export default router;