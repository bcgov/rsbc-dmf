import { Router, Request, Response } from 'express';
import { initializeTemplate } from '../services/initializeTemplate';
import { config } from '../config';

const router = Router();

// Template reload endpoint (can be used for automation or manual triggering)
router.post('/reload-template', async (_req: Request, res: Response): Promise<void> => {
  try {
    console.log('🔄 Reloading form template...');
    await initializeTemplate(config.TEMPLATE_URL); // Re-initialize the template
    res.status(200).send({ status: 'OK', message: 'Template reloaded.' });
  } catch (err) {
    console.error('❌ Failed to reload template:', err);
    res.status(500).send({ status: 'error', message: 'Failed to reload template.' });
  }
});

export default router;
